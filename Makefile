PROJECT = OsuRealmMerger.csproj

.PHONY: all tiny big release clean linux-tiny win-tiny mac-tiny linux-big win-big mac-big

all:
	dotnet build

release: tiny big
	@echo "Build complete! Check ./dist/ folder."

tiny: linux-tiny win-tiny mac-tiny

linux-tiny:
	dotnet publish $(PROJECT) -c Release -r linux-x64 --no-self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=false -o ./dist/tiny/linux

win-tiny:
	dotnet publish $(PROJECT) -c Release -r win-x64 --no-self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=false -o ./dist/tiny/win

mac-tiny:
	dotnet publish $(PROJECT) -c Release -r osx-arm64 --no-self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=false -o ./dist/tiny/mac

big: linux-big win-big mac-big

linux-big:
	dotnet publish $(PROJECT) -c Release -r linux-x64 --self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:PublishTrimmed=false -o ./dist/big/linux

win-big:
	dotnet publish $(PROJECT) -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:PublishTrimmed=false -o ./dist/big/win

mac-big:
	dotnet publish $(PROJECT) -c Release -r osx-arm64 --self-contained -p:PublishSingleFile=true -p:EnableCompressionInSingleFile=true -p:PublishTrimmed=false -o ./dist/big/mac

clean:
	rm -rf bin obj dist