locals {
  web_origin_id = "speakat-storage-092205142257-ap-northeast-2-an.s3.ap-northeast-2.amazonaws.com-mpw7z1mqhzn"
}

resource "aws_acm_certificate" "web" {
  provider          = aws.us_east_1
  domain_name       = "web.speakat.chokoring.com"
  validation_method = "DNS"
}

resource "aws_cloudfront_origin_access_control" "storage" {
  name                              = "oac-speakat-storage-092205142257-ap-northeast-2-an.s-mpw81iz8vd6"
  description                       = "Created by CloudFront"
  origin_access_control_origin_type = "s3"
  signing_behavior                  = "always"
  signing_protocol                  = "sigv4"
}

resource "aws_cloudfront_distribution" "web" {
  enabled         = true
  aliases         = ["web.speakat.chokoring.com"]
  http_version    = "http2"
  is_ipv6_enabled = true
  price_class     = "PriceClass_All"

  # 콘솔에서 배포를 만들 때 자동으로 생긴 WAF
  web_acl_id = "arn:aws:wafv2:us-east-1:092205142257:global/webacl/CreatedByCloudFront-5224fd38/bd05db2c-4043-4549-a9ea-e36358c45388"

  origin {
    origin_id                = local.web_origin_id
    domain_name              = aws_s3_bucket.storage.bucket_regional_domain_name
    origin_path              = "/web"
    origin_access_control_id = aws_cloudfront_origin_access_control.storage.id
  }

  default_cache_behavior {
    target_origin_id       = local.web_origin_id
    viewer_protocol_policy = "redirect-to-https"
    allowed_methods        = ["GET", "HEAD"]
    cached_methods         = ["GET", "HEAD"]
    compress               = true
    cache_policy_id        = "658327ea-f89d-4fab-a63d-7e88639e58f6" # Managed-CachingOptimized
  }

  restrictions {
    geo_restriction {
      restriction_type = "none"
    }
  }

  viewer_certificate {
    acm_certificate_arn      = aws_acm_certificate.web.arn
    ssl_support_method       = "sni-only"
    minimum_protocol_version = "TLSv1.2_2021"
  }

  tags = {
    Name = "speakat web"
  }
}
