up:
	pulumi stack init katasec/azure-hub/dev || true
	pulumi stack select katasec/azure-hub/dev
	pulumi up -f -y

destroy:
	pulumi destroy -y || true
	#pulumi destroy -s katasec/azure-hub/dev	
	pulumi stack rm katasec/azure-hub/dev || true
