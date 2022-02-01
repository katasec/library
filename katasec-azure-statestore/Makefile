up:
	pulumi stack init katasec/katasec-azure-statestore/dev || true
	pulumi stack select katasec/katasec-azure-statestore/dev
	pulumi up -f -y

destroy:
	pulumi destroy -y || true
	#pulumi destroy -s katasec/azure-statestore/dev || true
	pulumi stack rm katasec/katasec-azure-statestore/dev -y || true
