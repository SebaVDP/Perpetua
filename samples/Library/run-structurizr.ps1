# Runs Structurizr on http://localhost:8080 for samples\Library\docs\workspace.dsl (stop with Ctrl+C).
docker run -it --rm -p 8080:8080 `
  -v "${PSScriptRoot}\docs:/usr/local/structurizr" `
  structurizr/structurizr local
