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

## Publish

```
npm run publish
```

## Synth

Synth is useful during development to test out changes you're making to CDK. It doesn't actually deploy anything, it just
prints out the stack as YAML so that you can see what it will do at deployment time.

```
aws sso login --profile sg-dev
cdk synth sg-blog-api-1001-app --profile sg-dev
```

## Deploy

Step 1: Refresh your AWS credentials

```
aws sso login --profile sg-dev
```

Step 2: Deploy the app stack

```
cdk deploy sg-blog-api-1001-app --profile sg-dev
```

## Destroy

```
aws sso login --profile sg-dev
cdk destroy sg-blog-api-1001-app --profile sg-dev
```
