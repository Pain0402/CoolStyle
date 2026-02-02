# Debugging Agent Protocol

## 1. Context
This document defines the mindset and steps for the AI Agent (and the Developer) when debugging errors in the FashionEcommerce system.

## 2. The 5-Step Debugging Loop
When an error occurs (e.g., "API 404", "Build Failed"), follow this loop strictly:

### Step 1: Isolate the Layer (Locate)
- **Frontend?** Check Browser Console (F12) -> Network Tab.
  - *Status 500?* -> Issue is Backend.
  - *Status 400?* -> Issue is Payload/Validation.
  - *Status 0/Blocked?* -> Issue is CORS/Network.
- **Backend?** Check Seq Logs (http://localhost:5341) or Console Output.
  - *Exception?* -> Copy the Stack Trace.
  - *No Log?* -> Request never reached the middleware (Routing/Port issue).
- **Database?** Check Container Logs (`docker logs fashion_sql`).

### Step 2: Reproduce & Minimal Case
- Can I replicate this with a `curl` or `Postman` command?
- *Rule*: Never debug a complex UI flow if the API endpoint itself crashes on a simple GET request.

### Step 3: Analyze Dependencies
- Check `.csproj` versions. (Are we mixing .NET 8 and .NET 10?)
- Check `package.json`.
- Check `docker-compose.yml` status (`docker ps`).

### Step 4: Fix & Verify
- Apply the fix.
- **Crucial**: Restart the service. `dotnet run` needs a restart. `vite` usually HMRs but config changes need restart.
- Verify using the *Minimal Case* from Step 2.

### Step 5: Document
- What was the root cause?
- Add a comment in the code if the fix is not obvious (e.g., `// Required for CORS`).

## 3. Common Error Dictionary (Project Specific)

| Error | Likely Cause | Solution |
|-------|--------------|----------|
| `NU1605: Detected package downgrade` | SDK mismatch (.NET 10 Preview vs .NET 8) | Force version in `.csproj` or remove `bin/obj`. |
| `SqlException: Cannot open database` | Docker container not ready | Wait 30s. Check `docker ps`. |
| `Vite: connection reused` | Port conflict | Kill node processes. |
| `Swashbuckle 404` | Not in Development env | Check `ASPNETCORE_ENVIRONMENT`. |

## 4. Emergency Commands
- **Nuke DB & Restart**: `docker compose down -v && docker compose up -d`
- **Clean Backend**: `Get-ChildItem -Include bin,obj -Recurse | Remove-Item -Force -Recurse`
- **Clean Frontend**: `rm -rf node_modules && npm install`
