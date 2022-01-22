up:
	pulumi stack init katasec/katasec-azure-statestore/dev || true
	pulumi stack select katasec/katasec-azure-statestore/dev
	pulumi up -y

destroy:
	pulumi destroy -y || true
	pulumi stack rm katasec/dev || true