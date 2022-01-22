up:
	pulumi stack init katasec/azure-base-resourcegroups/dev || true
	pulumi stack select katasec/azure-base-resourcegroups/dev
	pulumi up -y

destroy:
	pulumi destroy -y || true
	pulumi stack rm katasec/dev || true