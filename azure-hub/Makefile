up:
	pulumi stack init katasec/azure-hub/dev || true
	pulumi stack select katasec/azure-hub/dev
	pulumi up -y

destroy:
	pulumi destroy -y || true
	pulumi stack rm katasec/dev || true