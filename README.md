<h1>Kiroll</h1>

Kiroll (whose name means "Keyboards Infinite Roll")
is a monophonic accessible digital musical instrument whose control is based on the use of the eyes and mouth. 
The focus then was on providing total controllability of the instrument through these two channels of interaction 
for full autonomous use of the same, even in the presence of severe motor limitations (e.g., quadriplegia).

The instrument was developed as a thesis project at the conclusion of the three-year course of study in 'Musical Informatics'.

<h2>Hardware needed</h2>

- Tobii Eye Tracker 5, eye tracker with which the instrument was tested and developed (https://gaming.tobii.com/product/eye-tracker-5).
- Breath sensor, buildable by following the instructions at the following link: https://neeqstock.notion.site/NithSensors-56ab43db493a423f9e8823af04fa9c46.

  The link refers to the website of PhD Davanzo Nicola who followed and helped me during the development of the project, in particular 
  to the section devoted to several sensors he made, including the breath sensor used in Kiroll (denoted NithBS on the site).
  On the site it will also be possible to access various information relating to the instruments developed by Nicola.

Even in the absence of the listed hardware, it will be possible to start the application using the mouse cursor, in place of visual pointing, 
for selecting notes, and the space bar, as an alternative to the breath sensor, for playing them.

<h2>Software required:</h2>

- Microsoft .NET Framework Runtime.
- Kiroll depends on a library developed once again by Davanzo Nicola, called NeeqDMIs, 
written specifically to make it faster and easier to develop accessible digital tools
from interaction channels on the musician's face (e.g., eyes, mouth...). 
Below the link to the GitHub repository: https://github.com/Neeqstock/NeeqDMIs.
- Kiroll does not contain an internal synthesizer; in fact, it represents a MIDI interface. It will therefore be necessary to create an internal MIDI loop on the PC to be exploited as a synthesizer or alternatively you can use an external hardware synthesizer connected to the PC sound card via a MIDI cable.
Please refer to the following guide: https://neeqstock.github.io/vst_guide/
