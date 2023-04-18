# sg-blog-api

## Prereqs

- Install VS Code for working in CDK in Typescript
- Install Node.js 16 LTS https://nodejs.org/en/
- Install cdk cli tool globally https://docs.aws.amazon.com/cdk/v2/guide/getting_started.html
- Install AWS cli tool https://aws.amazon.com/cli/ and setup default credentials using `aws configure`.
- Get familar with https://github.com/aws/aws-lambda-dotnet

## Install

Install CDK cli

```
npm install -g aws-cdk
```

Install cdk packages

```
npm i
```

Create a `.env` file and add the following variables:

```
CDK_DEFAULT_ACCOUNT=<AWS ACCOUNT ID>
CDK_DEFAULT_REGION=<AWS REGION>
SERVICE=sg-blog-api
STAGE=1001 # This is your ticket/feature branch name like 1001. We also use stages for dev, uat and prod.
```

Create an instance profile called `sg-dev`

```
aws configure sso --profile sg-dev
```

## Synth

Synth is useful during development to test out changes you're making to CDK. It doesn't actually deploy anything, it just
prints out the stack as YAML so that you can see what it will do at deployment time.

```
aws sso login --profile sg-dev
cdk synth sg-blog-api-feat-1008-app --profile sg-dev
```

## Deploy

Step 1: Refresh your AWS credentials

```
npm run sso
```

Step 2: Compile the lambda using AOT

```
npm run build
```

Step 3: Deploy

```
cdk deploy sg-blog-api-feat-1008-app --profile sg-dev
```

## Destroy

```
npm run sso
cdk destroy sg-blog-api-feat-1008-app --profile sg-dev
```
