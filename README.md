info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7109
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5229
info: Microsoft.Hosting.Lifetime[0]
      Application started. Press Ctrl+C to shut down.
info: Microsoft.Hosting.Lifetime[0]
      Hosting environment: Development
info: Microsoft.Hosting.Lifetime[0]
      Content root path: C:\Users\ZhivotovSS\Desktop\Web\Web
Запрос получен с токеном: c9a3a864-1510-4c84-883a-1c561fb9f512
warn: Microsoft.EntityFrameworkCore.Query[20504]
      Compiling a query which loads related collections for more than one collection navigation, either via 'Include' or through projection, but no 'QuerySplittingBehavior' has been configured. By default, Entity Framework will use 'QuerySplittingBehavior.SingleQuery', which can potentially result in slow query performance. See https://go.microsoft.com/fwlink/?linkid=2134277 for more information. To identify the query that's triggering this warning call 'ConfigureWarnings(w => w.Throw(RelationalEventId.MultipleCollectionIncludeWarning))'.
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (26ms) [Parameters=[@__token_0='?' (Size = 36) (DbType = AnsiString)], CommandType='Text', CommandTimeout='30']
      SELECT [q2].[id], [q2].[access_link_token], [q2].[created_at], [q2].[IsFavorite], [q2].[is_published], [q2].[title], [q2].[type_questionnaire_ID], [q2].[user_ID], [s].[id], [s].[Order], [s].[question_type_ID], [s].[questionnaire_ID], [s].[text], [s].[id0], [s].[option_text], [s].[order0], [s].[question_ID]
      FROM (
          SELECT TOP(1) [q].[id], [q].[access_link_token], [q].[created_at], [q].[IsFavorite], [q].[is_published], [q].[title], [q].[type_questionnaire_ID], [q].[user_ID]
          FROM [Questionnaire] AS [q]
          WHERE CONVERT(varchar(36), [q].[access_link_token]) = @__token_0 AND [q].[is_published] = CAST(1 AS bit)
      ) AS [q2]
      LEFT JOIN (
          SELECT [q0].[id], [q0].[Order], [q0].[question_type_ID], [q0].[questionnaire_ID], [q0].[text], [q1].[id] AS [id0], [q1].[option_text], [q1].[order] AS [order0], [q1].[question_ID]
          FROM [Question] AS [q0]
          LEFT JOIN [QuestionOption] AS [q1] ON [q0].[id] = [q1].[question_ID]
      ) AS [s] ON [q2].[id] = [s].[questionnaire_ID]
      ORDER BY [q2].[id], [s].[Order], [s].[id], [s].[order0]
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [Parameters=[@__accessLinkToken_0='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
      SELECT TOP(1) [q].[id], [q].[access_link_token], [q].[created_at], [q].[IsFavorite], [q].[is_published], [q].[title], [q].[type_questionnaire_ID], [q].[user_ID]
      FROM [Questionnaire] AS [q]
      WHERE [q].[access_link_token] = @__accessLinkToken_0
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (2ms) [Parameters=[@__questionId_0='?' (DbType = Int32), @__questionnaire_Id_1='?' (DbType = Int32)], CommandType='Text', CommandTimeout='30']
      SELECT [q1].[id], [q1].[Order], [q1].[question_type_ID], [q1].[questionnaire_ID], [q1].[text], [q0].[id], [q0].[option_text], [q0].[order], [q0].[question_ID]
      FROM (
          SELECT TOP(1) [q].[id], [q].[Order], [q].[question_type_ID], [q].[questionnaire_ID], [q].[text]
          FROM [Question] AS [q]
          WHERE [q].[id] = @__questionId_0 AND [q].[questionnaire_ID] = @__questionnaire_Id_1
      ) AS [q1]
      LEFT JOIN [QuestionOption] AS [q0] ON [q1].[id] = [q0].[question_ID]
      ORDER BY [q1].[id]
fail: Microsoft.EntityFrameworkCore.Database.Command[20102]
      Failed executing DbCommand (3ms) [Parameters=[@p0='?' (DbType = Int32), @p1='?' (DbType = Guid)], CommandType='Text', CommandTimeout='30']
      SET IMPLICIT_TRANSACTIONS OFF;
      SET NOCOUNT ON;
      INSERT INTO [Anonymous] ([CreatedAt], [session_id])
      OUTPUT INSERTED.[id]
      VALUES (@p0, @p1);
fail: Microsoft.EntityFrameworkCore.Update[10000]
      An exception occurred in the database while saving changes for context type 'Web.Models.QuestionnaireContext'.
      Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
       ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Invalid column name 'CreatedAt'.
         at Microsoft.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__211_0(Task`1 result)
         at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
      --- End of stack trace from previous location ---
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
         at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
      --- End of stack trace from previous location ---
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
      ClientConnectionId:165e6a5a-e1ea-45c6-ab02-6790dcf7795f
      Error Number:207,State:1,Class:16
         --- End of inner exception stack trace ---
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
      Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
       ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Invalid column name 'CreatedAt'.
         at Microsoft.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__211_0(Task`1 result)
         at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
      --- End of stack trace from previous location ---
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
         at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
      --- End of stack trace from previous location ---
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
      ClientConnectionId:165e6a5a-e1ea-45c6-ab02-6790dcf7795f
      Error Number:207,State:1,Class:16
         --- End of inner exception stack trace ---
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
fail: Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddleware[1]
      An unhandled exception has occurred while executing the request.
      Microsoft.EntityFrameworkCore.DbUpdateException: An error occurred while saving the entity changes. See the inner exception for details.
       ---> Microsoft.Data.SqlClient.SqlException (0x80131904): Invalid column name 'CreatedAt'.
         at Microsoft.Data.SqlClient.SqlCommand.<>c.<ExecuteDbDataReaderAsync>b__211_0(Task`1 result)
         at System.Threading.Tasks.ContinuationResultTaskFromResultTask`2.InnerInvoke()
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
      --- End of stack trace from previous location ---
         at System.Threading.ExecutionContext.RunInternal(ExecutionContext executionContext, ContextCallback callback, Object state)
         at System.Threading.Tasks.Task.ExecuteWithThreadLocal(Task& currentTaskSlot, Thread threadPoolThread)
      --- End of stack trace from previous location ---
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalCommand.ExecuteReaderAsync(RelationalCommandParameterObject parameterObject, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
      ClientConnectionId:165e6a5a-e1ea-45c6-ab02-6790dcf7795f
      Error Number:207,State:1,Class:16
         --- End of inner exception stack trace ---
         at Microsoft.EntityFrameworkCore.Update.ReaderModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Update.Internal.SqlServerModificationCommandBatch.ExecuteAsync(IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Update.Internal.BatchExecutor.ExecuteAsync(IEnumerable`1 commandBatches, IRelationalConnection connection, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.Storage.RelationalDatabase.SaveChangesAsync(IList`1 entries, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(IList`1 entriesToSave, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.ChangeTracking.Internal.StateManager.SaveChangesAsync(StateManager stateManager, Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.SqlServer.Storage.Internal.SqlServerExecutionStrategy.ExecuteAsync[TState,TResult](TState state, Func`4 operation, Func`4 verifySucceeded, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
         at Microsoft.EntityFrameworkCore.DbContext.SaveChangesAsync(Boolean acceptAllChangesOnSuccess, CancellationToken cancellationToken)
         at Web.Controllers.QuestionnaireController.GetUserIdAndAnonymousIdAsync() in C:\Users\ZhivotovSS\Desktop\Web\Web\Controllers\QuestionnaireController.cs:line 336
         at Web.Controllers.QuestionnaireController.SubmitAnswer(Guid accessLinkToken, Int32 questionId, AnswerRequest request) in C:\Users\ZhivotovSS\Desktop\Web\Web\Controllers\QuestionnaireController.cs:line 1149
         at Microsoft.AspNetCore.Mvc.Infrastructure.ActionMethodExecutor.TaskOfIActionResultExecutor.Execute(ActionContext actionContext, IActionResultTypeMapper mapper, ObjectMethodExecutor executor, Object controller, Object[] arguments)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeActionMethodAsync>g__Awaited|12_0(ControllerActionInvoker invoker, ValueTask`1 actionResultValueTask)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeNextActionFilterAsync>g__Awaited|10_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Rethrow(ActionExecutedContextSealed context)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.Next(State& next, Scope& scope, Object& state, Boolean& isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker.<InvokeInnerFilterAsync>g__Awaited|13_0(ControllerActionInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeFilterPipelineAsync>g__Awaited|20_0(ResourceInvoker invoker, Task lastTask, State next, Scope scope, Object state, Boolean isCompleted)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
         at Microsoft.AspNetCore.Mvc.Infrastructure.ResourceInvoker.<InvokeAsync>g__Awaited|17_0(ResourceInvoker invoker, Task task, IDisposable scope)
         at Microsoft.AspNetCore.Diagnostics.ExceptionHandlerMiddlewareImpl.<Invoke>g__Awaited|10_0(ExceptionHandlerMiddlewareImpl middleware, HttpContext context, Task task)
fail: Program[0]
      Ошибка сервера: 500
