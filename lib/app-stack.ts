import * as cdk from "aws-cdk-lib";
import * as apigateway from "aws-cdk-lib/aws-apigateway";
import * as cloudwatch from "aws-cdk-lib/aws-cloudwatch";
import * as actions from "aws-cdk-lib/aws-cloudwatch-actions";
import * as dynamodb from "aws-cdk-lib/aws-dynamodb";
import * as lambda from "aws-cdk-lib/aws-lambda";
import * as logs from "aws-cdk-lib/aws-logs";
import * as sns from "aws-cdk-lib/aws-sns";
import { Construct } from "constructs";

export interface AppStackProps extends cdk.StackProps {
  stage: string;
  service: string;
}

type RequiredPick<T, K extends keyof T> = Required<Pick<T, K>>;

type RequiredFunctionProps = RequiredPick<lambda.FunctionProps, "code" | "functionName"> &
  Partial<lambda.FunctionProps> & {
    alarmTopic: sns.Topic;
  };

export class AppStack extends cdk.Stack {
  private props: AppStackProps;

  constructor(scope: Construct, id: string, props: AppStackProps) {
    super(scope, id, props);
    this.props = props;

    const alarmTopic = new sns.Topic(this, "AlarmTopic", {
      topicName: `${this.stackName}-alarm`,
    });

    const blogTable = new dynamodb.Table(this, "BlogTable", {
      tableName: `${this.stackName}-blog`,
      billingMode: dynamodb.BillingMode.PAY_PER_REQUEST,
      encryption: dynamodb.TableEncryption.AWS_MANAGED,
      pointInTimeRecovery: true,
      partitionKey: {
        name: "Pk",
        type: dynamodb.AttributeType.STRING,
      },
      sortKey: {
        name: "Sk",
        type: dynamodb.AttributeType.STRING,
      },
      removalPolicy: cdk.RemovalPolicy.DESTROY,
    });

    const createPost = this.createLambda("CreatePost", {
      code: lambda.Code.fromAsset("./.build/SgBlogApi.CreatePost"),
      functionName: `${this.stackName}-CreatePost`,
      environment: {
        TABLE_NAME: blogTable.tableName,
      },
      alarmTopic,
    });

    const getPost = this.createLambda("GetPost", {
      code: lambda.Code.fromAsset("./.build/SgBlogApi.GetPost"),
      functionName: `${this.stackName}-GetPost`,
      environment: {
        TABLE_NAME: blogTable.tableName,
      },
      alarmTopic,
    });

    const updatePost = this.createLambda("UpdatePost", {
      code: lambda.Code.fromAsset("./.build/SgBlogApi.UpdatePost"),
      functionName: `${this.stackName}-UpdatePost`,
      environment: {
        TABLE_NAME: blogTable.tableName,
      },
      alarmTopic,
    });

    const deletePost = this.createLambda("DeletePost", {
      code: lambda.Code.fromAsset("./.build/SgBlogApi.DeletePost"),
      functionName: `${this.stackName}-DeletePost`,
      environment: {
        TABLE_NAME: blogTable.tableName,
      },
      alarmTopic,
    });

    const listPost = this.createLambda("ListPost", {
      code: lambda.Code.fromAsset("./.build/SgBlogApi.ListPost"),
      functionName: `${this.stackName}-ListPost`,
      environment: {
        TABLE_NAME: blogTable.tableName,
      },
      alarmTopic,
    });

    blogTable.grantReadWriteData(createPost);
    blogTable.grantReadData(getPost);
    blogTable.grantReadWriteData(updatePost);
    blogTable.grantReadWriteData(deletePost);
    blogTable.grantReadData(listPost);

    const api = new apigateway.RestApi(this, "ApiGateway", {
      restApiName: this.stackName,
      deployOptions: {
        stageName: "LIVE",
        tracingEnabled: true,
      },
      defaultCorsPreflightOptions: {
        allowMethods: apigateway.Cors.ALL_METHODS,
        allowOrigins: apigateway.Cors.ALL_ORIGINS,
        allowHeaders: apigateway.Cors.DEFAULT_HEADERS,
        maxAge: cdk.Duration.seconds(60),
      },
      cloudWatchRole: false,
    });

    const v1 = api.root.addResource("v1");
    const blogId = v1.addResource("{blogId}");

    const posts = blogId.addResource("posts");
    posts.addMethod("GET", new apigateway.LambdaIntegration(listPost));
    posts.addMethod("POST", new apigateway.LambdaIntegration(createPost));

    const postsPostId = posts.addResource("{postId}");
    postsPostId.addMethod("GET", new apigateway.LambdaIntegration(getPost));
    postsPostId.addMethod("PUT", new apigateway.LambdaIntegration(updatePost));
    postsPostId.addMethod("DELETE", new apigateway.LambdaIntegration(deletePost));
  }

  private createLambda(id: string, options: RequiredFunctionProps): lambda.Function {
    const lambdaFunction = new lambda.Function(this, id, {
      handler: "bootstrap",
      runtime: lambda.Runtime.PROVIDED_AL2,
      architecture: lambda.Architecture.X86_64,
      timeout: cdk.Duration.seconds(30),
      memorySize: 1024,
      ...options,
      environment: {
        SERVICE: this.props.service,
        STAGE: this.props.stage,
        ...options.environment,
      },
    });

    new logs.LogGroup(this, `${id}LogGroup`, {
      logGroupName: `/aws/lambda/${lambdaFunction.functionName}`,
      retention: this.props.stage === "prod" ? logs.RetentionDays.ONE_YEAR : logs.RetentionDays.ONE_WEEK,
      removalPolicy: cdk.RemovalPolicy.DESTROY,
    });

    const alarm = new cloudwatch.Alarm(this, `${id}Errors`, {
      alarmName: `${this.stackName}-${id}Errors`,
      comparisonOperator: cloudwatch.ComparisonOperator.GREATER_THAN_OR_EQUAL_TO_THRESHOLD,
      metric: lambdaFunction.metricErrors({ statistic: "Sum", period: cdk.Duration.minutes(1) }),
      threshold: 1,
      evaluationPeriods: 1,
      actionsEnabled: true,
    });

    alarm.addAlarmAction(new actions.SnsAction(options.alarmTopic));

    return lambdaFunction;
  }
}
