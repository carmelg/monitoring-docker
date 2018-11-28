From current folder, run:

docker build -t wpc2018/audified:0.1 .
docker run -i -v /var/run/docker.sock:/var/run/docker.sock wpc2018/audified:0.1