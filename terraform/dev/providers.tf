terraform {
  required_version = ">= 1.10" 

  required_providers {
    aws = {
      source  = "hashicorp/aws"
      version = "~> 6.0"
    }
  }

  backend "s3" {
    bucket       = "speakat-tfstate"
    key          = "dev/terraform.tfstate"
    region       = "ap-northeast-2"
    encrypt      = true
    use_lockfile = true
  }
}

provider "aws" {
  region = "ap-northeast-2"

  default_tags {
    tags = {
      Project   = "speakat"
      Stack     = "dev"
      ManagedBy = "terraform"
    }
  }
}

# CloudFront 가 붙는 ACM 인증서는 반드시 us-east-1 에 있어야 한다
provider "aws" {
  alias  = "us_east_1"
  region = "us-east-1"

  default_tags {
    tags = {
      Project   = "speakat"
      Stack     = "dev"
      ManagedBy = "terraform"
    }
  }
}
