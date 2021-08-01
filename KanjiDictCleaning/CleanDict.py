import xml.etree.ElementTree as ET
import codecs
import json
import sys

print("loading xml")
root = ET.parse('kanjidic2.xml').getroot()
print("xml loaded")

print("opening krad")
krad = codecs.open("kradfile-u.txt", encoding='utf-8')
print("krad opened")

if len(sys.argv) > 1:
	print("opening " + sys.argv[1])
	inputFile = codecs.open(sys.argv[1], encoding='utf-8')
else:
	print("No file given, so opening joyo")
	inputFile = codecs.open("joyo.txt", encoding='utf-8')

theKanji = []
def GenKanjiInfoString(kanji):
	meanings = " "
	for meaning in kanji.find('reading_meaning').find('rmgroup').findall('meaning'):
		if(meaning.attrib.get("m_lang") is None):
			meanings += meaning.text + "/"
	
	return meanings.split("/")

def GetKanjiEntry(kanji):
	for kanji in root.findall('character'):
		if(kanji.find('literal').text == i):
			return kanji
		else:
			continue
	return "not found"

def FindKanjiRadicals(kanji):
	for radEntry in krad:
		if radEntry[0] == kanji:
			krad.seek(0)
			return radEntry[4:].rstrip().split(" ")
	
	krad.seek(0)
	return "radicals not found"

def FindKanjiReadings(kanji):
	onyomi, kunyomi = " ", " "
	for reading in kanji.find('reading_meaning').find('rmgroup').findall('reading'):
		if reading.attrib.get("r_type") == "ja_on":
			onyomi += reading.text + "/"
		elif reading.attrib.get("r_type") == "ja_kun":
			kunyomi += reading.text + "/"

	return onyomi.split("/")[:-1], kunyomi.split("/")[:-1]

def CheckRadicalCount(cleanedEntries):
	radicalContents = []
	for kanji in cleanedEntries:
		for radical in kanji["radicals"]:
			if radical not in radicalContents:
				radicalContents.append(radical)
	
	for radical in radicalContents:
		print(radical)
	if len(radicalContents) > 7:
		print("WARNING: TOO MANY RADS!")

splitInputFile = inputFile.readline().split()
for i in splitInputFile:
	kanjiEntry = GetKanjiEntry(i)
	foundMeanings = GenKanjiInfoString(kanjiEntry)
	foundMeanings = [var.lstrip() for var in foundMeanings if var]
	foundRadicals = FindKanjiRadicals(i) #FindKanjiRadicals(i).rstrip().split(" ")
	onyomi, kunyomi = FindKanjiReadings(kanjiEntry)
	theKanji.append( {'kanji' : i, "meanings" : foundMeanings, "radicals" : foundRadicals, "onyomi" : onyomi, "kunyomi" : kunyomi})

outputFile = open("output.json", "w")
outputFile.writelines(json.dumps({"kanjiInfos" : theKanji}))
outputFile.close()
print(json.dumps({"kanjiInfos" : theKanji}))
CheckRadicalCount(theKanji)
#		misc = kanji.find('misc')
#		if(misc.find('jlpt') is not None):
#			if(misc.find('jlpt').text == i):
#				meanings = GenKanjiInfoString(kanji)
#				print(kanji.find('literal').text + meanings)


#for i in ['4']:
#	print("JLPT " + i)
#	print("=====================")
#	for kanji in root.findall('character'):
#		misc = kanji.find('misc')
#		if(misc.find('jlpt') is not None):
#			if(misc.find('jlpt').text == i):
#				meanings = GenKanjiInfoString(kanji)
#				print(kanji.find('literal').text + meanings)
			
	

    

