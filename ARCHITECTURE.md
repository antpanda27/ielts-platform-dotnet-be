# Architecture Overview

## System Architecture

```
┌─────────────────────────────────────────────────────────────────────┐
│                         DEVELOPMENT                                  │
│                                                                      │
│  ┌──────────────┐      ┌──────────────┐      ┌──────────────┐     │
│  │   Aspire     │      │   API        │      │   Web        │     │
│  │   AppHost    │─────▶│   Service    │◀─────│   Frontend   │     │
│  │              │      │              │      │   (Blazor)   │     │
│  └──────────────┘      └──────────────┘      └──────────────┘     │
│         │                      │                      │            │
│         │                      │                      │            │
│         └──────────────────────┴──────────────────────┘            │
│                                │                                    │
│                                ▼                                    │
│                        .NET 9 Runtime                               │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                                 │ Build & Test
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                           CI/CD PIPELINE                             │
│                                                                      │
│  ┌──────────┐    ┌──────────┐    ┌──────────┐    ┌──────────┐    │
│  │  Build   │───▶│   Test   │───▶│  Docker  │───▶│   Push   │    │
│  │          │    │          │    │  Build   │    │   to ECR │    │
│  └──────────┘    └──────────┘    └──────────┘    └──────────┘    │
│                                                          │          │
│                                                          ▼          │
│                                                  ┌──────────────┐  │
│                                                  │  Terraform   │  │
│                                                  │   Deploy     │  │
│                                                  └──────────────┘  │
└─────────────────────────────────────────────────────────────────────┘
                                 │
                                 │ Deploy
                                 ▼
┌─────────────────────────────────────────────────────────────────────┐
│                          AWS INFRASTRUCTURE                          │
│                                                                      │
│  ┌───────────────────────────────────────────────────────────────┐ │
│  │                         VPC (10.0.0.0/16)                      │ │
│  │                                                                │ │
│  │  ┌────────────────┐                  ┌────────────────┐      │ │
│  │  │ Public Subnet  │                  │ Public Subnet  │      │ │
│  │  │  AZ-1          │                  │  AZ-2          │      │ │
│  │  │                │                  │                │      │ │
│  │  │  ┌──────┐      │                  │  ┌──────┐      │      │ │
│  │  │  │ ALB  │◀─────┼──────────────────┼─▶│ ALB  │      │      │ │
│  │  │  └───┬──┘      │                  │  └───┬──┘      │      │ │
│  │  │      │         │                  │      │         │      │ │
│  │  │  ┌───▼──┐      │                  │  ┌───▼──┐      │      │ │
│  │  │  │ NAT  │      │                  │  │ NAT  │      │      │ │
│  │  │  │  GW  │      │                  │  │  GW  │      │      │ │
│  │  │  └───┬──┘      │                  │  └───┬──┘      │      │ │
│  │  └──────┼─────────┘                  └──────┼─────────┘      │ │
│  │         │                                    │                │ │
│  │         │                                    │                │ │
│  │  ┌──────▼──────────┐                ┌───────▼──────────┐     │ │
│  │  │ Private Subnet  │                │ Private Subnet   │     │ │
│  │  │  AZ-1           │                │  AZ-2            │     │ │
│  │  │                 │                │                  │     │ │
│  │  │  ┌──────────┐   │                │  ┌──────────┐   │     │ │
│  │  │  │   ECS    │   │                │  │   ECS    │   │     │ │
│  │  │  │  Tasks   │   │                │  │  Tasks   │   │     │ │
│  │  │  │ (Fargate)│   │                │  │ (Fargate)│   │     │ │
│  │  │  └─────┬────┘   │                │  └─────┬────┘   │     │ │
│  │  │        │        │                │        │        │     │ │
│  │  └────────┼────────┘                └────────┼────────┘     │ │
│  │           │                                   │              │ │
│  └───────────┼───────────────────────────────────┼──────────────┘ │
│              │                                   │                │
│              └───────────────┬───────────────────┘                │
│                              │                                    │
│                              ▼                                    │
│              ┌───────────────────────────────┐                   │
│              │      AWS Services              │                   │
│              │                                │                   │
│              │  ┌──────┐  ┌──────┐  ┌──────┐ │                   │
│              │  │  S3  │  │ Dyna │  │  ECR │ │                   │
│              │  │      │  │  moDB│  │      │ │                   │
│              │  └──────┘  └──────┘  └──────┘ │                   │
│              │                                │                   │
│              │  ┌────────────────────────┐   │                   │
│              │  │   CloudWatch Logs      │   │                   │
│              │  └────────────────────────┘   │                   │
│              └───────────────────────────────┘                   │
│                                                                   │
└───────────────────────────────────────────────────────────────────┘
```

## Component Details

### Application Layer
- **API Service**: ASP.NET Core Web API (.NET 9)
  - RESTful endpoints
  - Health checks
  - OpenAPI/Swagger integration
  - AWS SDK integration

- **Aspire AppHost**: Orchestration and service discovery
  - Local development dashboard
  - Service configuration
  - Health monitoring

- **Service Defaults**: Shared configurations
  - OpenTelemetry
  - Health checks
  - Service discovery

### Infrastructure Layer (AWS)

#### Networking
- **VPC**: Isolated network (10.0.0.0/16)
- **Public Subnets**: Internet-facing resources (ALB, NAT)
- **Private Subnets**: Application tier (ECS tasks)
- **Multi-AZ**: High availability across availability zones

#### Compute
- **ECS Fargate**: Serverless container platform
  - No server management
  - Auto-scaling capability
  - Container orchestration

#### Storage & Data
- **ECR**: Container image registry
  - Image versioning
  - Security scanning
  - Lifecycle policies

- **S3**: Object storage
  - File uploads
  - Static assets
  - Backups

- **DynamoDB**: NoSQL database
  - High performance
  - Scalable
  - Managed service

#### Monitoring
- **CloudWatch Logs**: Centralized logging
- **Container Insights**: ECS metrics
- **Health Checks**: Multi-level monitoring

### CI/CD Pipeline

#### Jenkins Stages
1. **Source Control**: Git checkout
2. **Build**: Compile .NET solution
3. **Test**: Run unit/integration tests
4. **Containerize**: Build Docker image
5. **Publish**: Push to ECR
6. **Infrastructure**: Terraform apply
7. **Deploy**: Update ECS service

## Data Flow

### Request Flow
```
User Request
    ↓
Internet
    ↓
Application Load Balancer (ALB)
    ↓
Target Group (Health Checked)
    ↓
ECS Service (Fargate Tasks)
    ↓
API Service Container
    ↓
AWS Services (S3, DynamoDB)
    ↓
Response to User
```

### Deployment Flow
```
Code Push
    ↓
Jenkins Webhook
    ↓
Build & Test
    ↓
Docker Build
    ↓
Push to ECR
    ↓
Terraform Plan/Apply
    ↓
ECS Service Update
    ↓
Rolling Deployment
    ↓
Health Check Validation
    ↓
Deployment Complete
```

## Security Architecture

### Network Security
- Private subnets for application tier
- Security groups restricting traffic
- NACLs for subnet-level control
- No public IPs on application containers

### Access Control
- IAM roles for ECS tasks
- Least privilege permissions
- No hardcoded credentials
- AWS Secrets Manager ready

### Data Security
- Encryption at rest (S3, EBS)
- Encryption in transit (HTTPS)
- VPC isolation
- Security group restrictions

## Scalability

### Horizontal Scaling
- ECS service desired count adjustment
- Auto-scaling based on metrics
- Multi-AZ distribution
- Load balancer distribution

### Vertical Scaling
- Fargate CPU/memory adjustment
- Container resource limits
- Database capacity adjustments

## High Availability

### Multi-AZ Design
- Resources spread across AZs
- Independent failure domains
- Automatic failover
- Load balancing across zones

### Health Monitoring
- ALB health checks
- ECS task health checks
- Container-level health checks
- CloudWatch alarms

## Cost Optimization

### Compute
- Fargate Spot for non-production
- Right-sized task definitions
- Auto-scaling to match demand

### Storage
- S3 lifecycle policies
- ECR image cleanup
- DynamoDB on-demand billing

### Networking
- Single NAT Gateway per AZ option
- VPC endpoints for AWS services (optional)
