FROM node:20-bookworm

RUN mkdir -p /home/tester
WORKDIR /home/tester
RUN npm install -g @apideck/portman newman newman-reporter-html
COPY script.sh .
RUN chmod 700 script.sh

# ENTRYPOINT [ "/bin/bash" ]
# CMD [ "script.sh" ]
ENTRYPOINT [ "bash", "script.sh" ]