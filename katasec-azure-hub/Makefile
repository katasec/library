up:
	pulumi stack init katasec/katasec-azure-hub/dev || true
	pulumi stack select katasec/katasec-azure-hub/dev
	pulumi up -f -y

destroy:
	pulumi destroy -y || true
	#pulumi destroy -s katasec/katasec-azure-hub/dev	
	pulumi stack rm katasec/katasec-azure-hub/dev -y || true
