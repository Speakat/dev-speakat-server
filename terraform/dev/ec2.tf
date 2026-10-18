resource "aws_instance" "api" {
  ami                    = "ami-084a56dceed3eb9bb"
  instance_type          = "t3a.micro"
  subnet_id              = "subnet-08d4e716f7647edb9"
  vpc_security_group_ids = [aws_security_group.api.id]
  iam_instance_profile   = aws_iam_instance_profile.ec2.name
  key_name               = "dev-speakat-server"

  # t3 계열 기본값은 unlimited 
  credit_specification {
    cpu_credits = "standard"
  }

  # IMDSv2 만 허용. 홉 2 는 컨테이너가 인스턴스 프로파일을 읽을 수 있게
  metadata_options {
    http_endpoint               = "enabled"
    http_tokens                 = "required"
    http_put_response_hop_limit = 2
  }

  root_block_device {
    volume_type = "gp3"
    volume_size = 16
    encrypted   = true
  }

  tags = {
    Name = "dev-speakat-server"
  }
}

resource "aws_eip" "api" {
  domain   = "vpc"
  instance = aws_instance.api.id
}

resource "aws_security_group" "api" {
  name        = "SpeakatServerGroup"
  description = "EC2"
  vpc_id      = "vpc-058c46af86b6d2f04"

  # TODO: 본인 IP 로 제한
  ingress {
    from_port   = 22
    to_port     = 22
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 80
    to_port     = 80
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  ingress {
    from_port   = 443
    to_port     = 443
    protocol    = "tcp"
    cidr_blocks = ["0.0.0.0/0"]
  }

  # TODO: sg-066da295db2e06173 확인
  ingress {
    from_port       = 5000
    to_port         = 5000
    protocol        = "tcp"
    security_groups = ["sg-066da295db2e06173"]
  }

  egress {
    from_port   = 0
    to_port     = 0
    protocol    = "-1"
    cidr_blocks = ["0.0.0.0/0"]
  }
}
