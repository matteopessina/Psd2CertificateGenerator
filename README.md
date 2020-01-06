# Generate Test PSD2 Certificates

Based on https://www.etsi.org/deliver/etsi_ts/119400_119499/119495/01.03.02_60/ts_119495v010302p.pdf

# Getting started

* Make sure you have .Net Core 3.0 installed.
1. **Clone** repository (`git clone https://github.com/payoneer/Psd2CertificateGenerator.git`)  
Inside the repo folder
1. **Build**  
`dotnet build`
1. **Run**. For example:  
`dotnet run --project=Psd2CertificateGenerator.CLI -n "My TPP Name" -a "PSDXX-YYY-ZZZZZZ" --country FR`
* You can run `dotnet run --project=Psd2CertificateGenerator.CLI` to see all the possible arguments.

## Using Docker
1. Build the docker image  
`docker build . -t payoneer/psd2gen`
1. Run it in a container  
`docker run -it --rm payoneer/psd2gen -n "My TPP Name" -a "PSDXX-YYY-ZZZZZZ" --country FR`
1. **Optional:** Create certificate files using a mount:  
    1. Create an output directory `mkdir out`
    1. Run the following: `docker run -it --mount type=bind,src=$PWD/out,dst=/out --rm payoneer/psd2gen -f /out/sample -n "My TPP Name" -a "PSDXX-YYY-ZZZZZZ" --country FR`

### Supported arguments
| Short | Argument | Description | Required | Default |
|-------|----------|-------------|----------|----------|
| -n | --name | TPP's name | * | |
| -a | --auth | TPP's PSD2 Authentication Number | * | |
| -c | country | TPP's country | * | |
| -f | --file | Output filename(s) (e.g. file.crt, file.key, file.pub) | | stdout | 
| -t | --type | The certificate type | | QWAC |
| -r | --roles | PSD2 roles | | PSP_AI_PSP_PI |
| -d | --issuer-dns | The Issuer's DNS name | | payoneer.com |
| -e| --issuer-email | The Issuer's Email address | | support@payoneer.com |
| -m| --issuer-cn | The Issuer's Common name | | Payoneer PSD2 Sandbox |
| -o| --issuer-o | The Issuer's organization name | | Payoneer |
| -u| --issuer-ou | The Issuer's organization unit name | | Payoneer EU |
| -l| --issuer-l | The Issuer's locality | | Gibraltar |
| -s| --issuer-s | The Issuer's state | | |
| -y| --issuer-c | The Issuer's country | | GI |
|  |--nca-name | The approving NCA's name | | Payoneer |
| | --nca-id | The approving NCA's id | | IL-PAY |
| | --help | Display help screen | | |
| | --version | Display version information | | |
