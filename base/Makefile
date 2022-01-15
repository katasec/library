up:
	pulumi stack init katasec/katasec-azure-base/dev || true
	pulumi stack select katasec/katasec-azure-base/dev
	pulumi up -y

destroy:
	pulumi destroy -y
	pulumi stack rm katasec/dev