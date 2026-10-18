resource "aws_s3_bucket" "storage" {
  bucket           = "speakat-storage-092205142257-ap-northeast-2-an"
  bucket_namespace = "account-regional"
}

# 아래 공개 읽기 정책이 동작하려면 BlockPublicPolicy / RestrictPublicBuckets 가 꺼져 있어야 한다
resource "aws_s3_bucket_public_access_block" "storage" {
  bucket                  = aws_s3_bucket.storage.id
  block_public_acls       = true
  ignore_public_acls      = true
  block_public_policy     = false
  restrict_public_buckets = false
}

resource "aws_s3_bucket_policy" "storage" {
  bucket = aws_s3_bucket.storage.id

  policy = jsonencode({
    Version = "2012-10-17"
    Statement = [
      {
        # TODO: web/* 는 CloudFront 로만 열고 공개 읽기에서 뺄지 결정
        Effect    = "Allow"
        Principal = "*"
        Action    = "s3:GetObject"
        Resource = [
          "${aws_s3_bucket.storage.arn}/profile-images/*",
          "${aws_s3_bucket.storage.arn}/web/*",
        ]
      },
      {
        Sid       = "AllowCloudFrontServicePrincipal"
        Effect    = "Allow"
        Principal = { Service = "cloudfront.amazonaws.com" }
        Action    = "s3:GetObject"
        Resource  = "${aws_s3_bucket.storage.arn}/*"
        Condition = {
          ArnLike = { "AWS:SourceArn" = aws_cloudfront_distribution.web.arn }
        }
      },
    ]
  })

  depends_on = [aws_s3_bucket_public_access_block.storage]
}
