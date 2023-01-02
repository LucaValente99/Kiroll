<h1>Kiroll</h1>

Kiroll (il cui nome significa “Keyboards Infinite Roll”, tradotto “Rotolo Infinito di Tastiere”)
è uno strumento musicale digitale accessibile monofonico il cui controllo è basato sull’utilizzo degli occhi e della bocca. 
L’obiettivo quindi si è focalizzato sul fornire la totale controllabilità dello strumento tramite questi due canali di interazione 
per un utilizzo dello stesso in totale autonomia, anche in presenza di gravi limiti motori (es. tetraplegia).

Lo strumento è stato sviluppato come progetto di tesi in conclusione del percorso di studi triennale in 'Informatica Musicale'.

<h2>Hardware necessario</h2>

- Tobii Eye Tracker 5, eye tracker con il quale è stato testato e sviluppato lo strumento (https://gaming.tobii.com/product/eye-tracker-5).
- Sensore a fiato, costruibile seguendo le istruzioni al link seguente: https://neeqstock.notion.site/NithSensors-56ab43db493a423f9e8823af04fa9c46.

  Il link rimanda al sito del PhD Davanzo Nicola che mi ha seguito e aiutato durante lo sviluppo del progetto, in particolare 
  alla sezione dedicata a diversi sensori da lui realizzati, tra cui il sensore a fiato utilizzato in Kiroll (indicata con NithBS nel sito).
  Sul sito sarà possibile inoltre accedere a diverese informazioni, relative agli strumenti da lui sviluppati.

Anche in assenza dell'hardware elencato sarà possibile avviare l'applicativo sfruttando il cursore del mouse, in sostituzione al puntamento visivo, 
per la selezione delle note, e la barra spaziatrice, in alternativa al sensore a fiato, per farle suonare.

<h2>Software necessario:</h2>

- Microsoft .NET Framework Runtime.
- Kiroll dipende da una libreria sviluppata ancora una volta da Davanzo Nicola, denominata NeeqDMIs, 
scritta appositamente per rendere pià veloce ed agevole lo sviluppo di strumenti digitali accessibili a partire 
da canali di interazione sul volto del musicista (es. occhi, bocca...). 
Di seguito il link al repository GitHub: https://github.com/Neeqstock/NeeqDMIs.
- Kiroll non contiene un sintetizzatore interno, rappresenta infatti un'interfaccia MIDI. Sarà quindi necessario creare un loop MIDI interno al PC da sfruttare 
come sintetizzatore o in alternativa è possibile utilizzare un sintetizzatore hardware esterno connesso alla scheda audio del PC tramite un cavo MIDI.
Anche in questo caso si rimanda ad una giuda: https://neeqstock.github.io/vst_guide/
