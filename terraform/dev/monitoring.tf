variable "monitoring_ssh_cidrs" {
  description = "모니터링 서버 SSH(터널링) 허용 CIDR"
  type        = list(string)

  validation {
    condition = (
      length(var.monitoring_ssh_cidrs) > 0 &&
      alltrue([for cidr in var.monitoring_ssh_cidrs : !contains(["0.0.0.0/0", "::/0"], cidr)])
    )
    error_message = "monitoring_ssh_cidrs에는 팀원의 제한된 CIDR만 설정해야 합니다. 전체 공개 CIDR은 허용하지 않습니다."
  }
}

locals {
  # aws_instance.api 를 참조하면 앱 서버 변경 시 data 읽기가 apply 로 밀려 EBS 교체 plan 발생
  monitoring_subnet_id = "subnet-08d4e716f7647edb9"
}

data "aws_subnet" "monitoring" {
  id = local.monitoring_subnet_id
}

data "aws_ami" "ubuntu_arm64" {
  most_recent = true
  owners      = ["099720109477"] # Canonical

  filter {
    name   = "name"
    values = ["ubuntu/images/hvm-ssd-gp3/ubuntu-noble-24.04-arm64-server-*"]
  }
}

resource "aws_instance" "monitoring" {
  ami                    = data.aws_ami.ubuntu_arm64.id
  instance_type          = "t4g.small"
  subnet_id              = local.monitoring_subnet_id
  vpc_security_group_ids = [aws_security_group.monitoring.id]
  iam_instance_profile   = aws_iam_instance_profile.monitoring.name
  key_name               = "dev-speakat-server"

  credit_specification {
    cpu_credits = "standard"
  }

  metadata_options {
    http_endpoint               = "enabled"
    http_tokens                 = "required"
    http_put_response_hop_limit = 1
  }

  root_block_device {
    volume_type = "gp3"
    volume_size = 16
    encrypted   = true
  }

  user_data = templatefile("${path.module}/templates/monitoring-init.sh.tftpl", {
    data_volume_id = aws_ebs_volume.monitoring_data.id
  })

  # AMI 갱신, user_data 수정 시 인스턴스 교체 방지 (user_data 변경은 서버에 직접 반영)
  lifecycle {
    ignore_changes = [ami, user_data]
  }

  tags = {
    Name = "dev-speakat-monitoring"
  }
}

# 인스턴스 교체에도 데이터 유지를 위해 루트 볼륨과 분리
resource "aws_ebs_volume" "monitoring_data" {
  availability_zone = data.aws_subnet.monitoring.availability_zone
  type              = "gp3"
  size              = 20
  encrypted         = true

  lifecycle {
    prevent_destroy = true
  }

  tags = {
    Name = "dev-speakat-monitoring-data"
  }
}

resource "aws_volume_attachment" "monitoring_data" {
  device_name = "/dev/sdf"
  volume_id   = aws_ebs_volume.monitoring_data.id
  instance_id = aws_instance.monitoring.id

  # 마운트 상태 강제 분리로 인한 파일시스템 손상 방지
  stop_instance_before_detaching = true
}

resource "aws_eip" "monitoring" {
  domain   = "vpc"
  instance = aws_instance.monitoring.id

  tags = {
    Name = "dev-speakat-monitoring"
  }
}

resource "aws_security_group" "monitoring" {
  name        = "SpeakatMonitoringGroup"
  description = "Monitoring EC2"
  vpc_id      = data.aws_subnet.monitoring.vpc_id

  # Grafana 는 SSH 터널 전용이라 3000 미개방
  ingress {
    description = "SSH"
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = var.monitoring_ssh_cidrs
  }

  ingress {
    description     = "Prometheus remote write from API"
    from_port       = 9090
    to_port         = 9090
    protocol        = "tcp"
    security_groups = [aws_security_group.api.id]
  }

  ingress {
    description     = "Loki push from API"
    from_port       = 3100
    to_port         = 3100
    protocol        = "tcp"
    security_groups = [aws_security_group.api.id]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}

resource "aws_iam_role" "monitoring" {
  name = "speakat-monitoring"

  assume_role_policy = jsonencode({
    Version = "2012-10-17"
    Statement = [{
      Effect    = "Allow"
      Action    = "sts:AssumeRole"
      Principal = { Service = "ec2.amazonaws.com" }
    }]
  })
}

resource "aws_iam_instance_profile" "monitoring" {
  name = aws_iam_role.monitoring.name
  role = aws_iam_role.monitoring.name
}

# SSH 불가 시 Session Manager 접속용
resource "aws_iam_role_policy_attachment" "monitoring_ssm" {
  role       = aws_iam_role.monitoring.name
  policy_arn = "arn:aws:iam::aws:policy/AmazonSSMManagedInstanceCore"
}

output "monitoring_public_ip" {
  description = "SSH 터널 접속 주소"
  value       = aws_eip.monitoring.public_ip
}

output "monitoring_private_ip" {
  description = "앱 서버 Alloy 전송 대상 주소"
  value       = aws_instance.monitoring.private_ip
}
