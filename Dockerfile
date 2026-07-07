# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# The runtime stage below is alpine (musl), so map BuildKit's TARGETARCH to the matching musl RID.
# The RID is passed on the command line only - MarkdownInk.Mcp.csproj intentionally sets no
# RuntimeIdentifier(s) (it would break the PackAsTool NuGet package; see the comment there).
ARG TARGETARCH
RUN arch="${TARGETARCH:-$(uname -m)}" \
    && case "$arch" in \
         amd64|x86_64)  rid=linux-musl-x64 ;; \
         arm64|aarch64) rid=linux-musl-arm64 ;; \
         *) echo "Unsupported architecture: $arch" >&2; exit 1 ;; \
       esac \
    && echo "$rid" > /tmp/rid

# Copy solution and project files for layer caching
COPY MarkdownInk.slnx ./
COPY src/MarkdownInk/MarkdownInk.csproj src/MarkdownInk/
COPY src/MarkdownInk.Mcp/MarkdownInk.Mcp.csproj src/MarkdownInk.Mcp/
COPY tests/MarkdownInk.Tests/MarkdownInk.Tests.csproj tests/MarkdownInk.Tests/
COPY tests/MarkdownInk.Mcp.Tests/MarkdownInk.Mcp.Tests.csproj tests/MarkdownInk.Mcp.Tests/

# Restore just the server project. PublishReadyToRun must also be set here so the crossgen2
# compiler pack is acquired for the --no-restore publish below.
RUN dotnet restore src/MarkdownInk.Mcp/MarkdownInk.Mcp.csproj -r "$(cat /tmp/rid)" -p:PublishReadyToRun=true

# Copy source
COPY . .

# Publish framework-dependent but RID-specific, with ReadyToRun (AOT-precompiled native code
# alongside the IL) to cut JIT work on cold start.
RUN dotnet publish src/MarkdownInk.Mcp/MarkdownInk.Mcp.csproj \
    -c Release \
    --no-restore \
    -r "$(cat /tmp/rid)" \
    --self-contained false \
    -p:PublishReadyToRun=true \
    -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/runtime:10.0-alpine AS runtime
WORKDIR /app

# icu-libs is needed for globalization (syntax highlighting / text handling).
RUN apk add --no-cache icu-libs

ENV DOTNET_SYSTEM_GLOBALIZATION_INVARIANT=false

COPY --from=build /app/publish .

# Run as an unprivileged user rather than root
RUN addgroup -S mdink \
    && adduser -S mdink -G mdink \
    && chown -R mdink:mdink /app
USER mdink

ENTRYPOINT ["dotnet", "MarkdownInk.Mcp.dll"]
