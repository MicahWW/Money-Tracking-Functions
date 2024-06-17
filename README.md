# Money Tracking Functions

## Purpose
I started this project because I wanted to my money is being spent / moving around. While there are many tools that do this many of them require payment for the features that are actually helpful while their free features are almost useless, this project will hopfully fix that by making a helpful tool for free. Also, many of those tools require you to share all of your data with them, this will be able to run self-hosted.

## Project State
In the current state of this project it leaves a lot to be desired. The code is horribly documented, the OpenAPI/swagger documentation is not guaranteed to be accurate, many features could be added etc.

On the topic of the OpenAPI documentation, a few different methods of ensuring that the code follows the documentation are being considered but at the moment they all require substaintial work to do so for the time being that is not being focused and will become a focus later on in the project. Some of the methods thought of are:
- Build a test suite with a Python library
  - This could be it's own full scale project as it could very easily be useful for anyone trying to do the same thing
  - There are some libraries out there right now have some simular features:
    - https://github.com/pb33f/wiretap
- Use PostMan to validate as it has testing features.
- Scrub through more of the tools listed at https://openapi.tools/
  - https://github.com/apideck-libraries/portman