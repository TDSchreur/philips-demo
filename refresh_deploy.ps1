$tag = [int](Get-Date -UFormat %s -Millisecond 0)
# helm uninstall philips-demo -n localdev
docker build -t host.docker.internal:5000/philips-api-localbuild:$tag .
docker push host.docker.internal:5000/philips-api-localbuild:$tag
helm upgrade --install philips-demo -n localdev --set ingress.path="/philips-api-local" --set image.repository="host.docker.internal:5000/philips-api-localbuild" --set image.tag=$tag .chart