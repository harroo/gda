
output:
	mcs -recurse:source/server/*.cs -out:build/gda-server.cil
	mcs -recurse:source/client/*.cs -out:build/gdacli

ready:
	echo "getting ready for build"
	mkdir build
