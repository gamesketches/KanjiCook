# coding: utf-8
import xml.etree.ElementTree as ET
import codecs
import json
import sys

maxLevelRadicals = 7
maxLevelKanji = 5
minLevelKanji = 4

print("loading xml")
root = ET.parse('kanjidic2.xml').getroot()
print("xml loaded")

print("opening krad")
krad = codecs.open("kradfile-u.txt", encoding='utf-8')
print("krad opened")



theKanji = []
def GenKanjiInfoString(kanji):
	meanings = " "
	for meaning in kanji.find('reading_meaning').find('rmgroup').findall('meaning'):
		if(meaning.attrib.get("m_lang") is None):
			meanings += meaning.text + "/"
	
	return meanings.split("/")

def GetKanjiEntry(kanjiToFind):
	for kanji in root.findall('character'):
		if(kanji.find('literal').text == kanjiToFind):
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
	if len(radicalContents) > maxLevelRadicals:
		print("WARNING: TOO MANY RADS!")

def GenFromFile(inputFile):
	splitInputFile = inputFile.readline().split()
	for i in splitInputFile:
		kanjiEntry = GetKanjiEntry(i)
		foundMeanings = GenKanjiInfoString(kanjiEntry)
		foundMeanings = [var.lstrip() for var in foundMeanings if var]
		foundRadicals = FindKanjiRadicals(i) 
		onyomi, kunyomi = FindKanjiReadings(kanjiEntry)
		theKanji.append( {'kanji' : i, "meanings" : foundMeanings, "radicals" : foundRadicals, "onyomi" : onyomi, "kunyomi" : kunyomi})

	outputFile = open("output.json", "w")
	outputFile.writelines(json.dumps({"kanjiInfos" : theKanji}))
	outputFile.close()
	print(json.dumps({"kanjiInfos" : theKanji}))
	CheckRadicalCount(theKanji)

def GenFromList(inputList):
	for i in inputList:
		kanjiEntry = GetKanjiEntry(i)
		foundMeanings = GenKanjiInfoString(kanjiEntry)
		foundMeanings = [var.lstrip() for var in foundMeanings if var]
		foundRadicals = FindKanjiRadicals(i) 
		onyomi, kunyomi = FindKanjiReadings(kanjiEntry)
		theKanji.append( {'kanji' : i, "meanings" : foundMeanings, "radicals" : foundRadicals, "onyomi" : onyomi, "kunyomi" : kunyomi})

	outputFile = open("output.json", "w")
	outputFile.writelines(json.dumps({"kanjiInfos" : theKanji}))
	outputFile.close()
	print(json.dumps({"kanjiInfos" : theKanji}))
	CheckRadicalCount(theKanji)

def GenFromKanji(startingLiteral):
	levelRadicals = []
	levelKanji = []
	levelKanji.append(startingLiteral)
	startingKanjiEntry = GetKanjiEntry(startingLiteral)
	startingRadicals = FindKanjiRadicals(startingLiteral)
	radSet = set(startingRadicals)
	levelRadicals = startingRadicals
	idleCounter = 0
	for kanji in root.findall('character'):
		freq = kanji.find("misc").find("freq")
		idleCounter += 1
		if idleCounter % 500 == 0:
			print(idleCounter)
		if freq is None:
			continue
		newRads = FindKanjiRadicals(kanji.find("literal").text)
		numIntersection = len(radSet.intersection(newRads))
		if numIntersection > 0:
			radsAdded = len(newRads) - numIntersection
			if (len(startingRadicals) + radsAdded) < maxLevelRadicals:
				startingRadicals += newRads
				radSet = set(startingRadicals)
				levelKanji.append(kanji.find("literal").text)
				print(kanji.find("literal").text)
				if len(levelKanji) >= maxLevelKanji:
					break
	print("finished checking")
	print(levelKanji)
	print(len(radSet))

def FindContentRecursively(curKanjiList, curRadicalSet):
	for kanji in root.findall('character'):
		freq = kanji.find("misc").find("freq")
		if freq is None:
			continue
		theKanji = kanji.find("literal").text
		if theKanji in curKanjiList:
			continue
		newRads = FindKanjiRadicals(theKanji)
		numIntersection = len(curRadicalSet.intersection(newRads))
		if numIntersection > 0:
			radsAdded = len(newRads) - numIntersection
			if len(curRadicalSet) + radsAdded < maxLevelRadicals:
				if len(curKanjiList) == maxLevelKanji - 1:
					print(curKanjiList)
					print(curRadicalSet.union(newRads))
					GenFromList(curKanjiList + [theKanji])
					return curKanjiList + [theKanji], curRadicalSet.union(newRads)
				else:
					print(".")
					newKanji, newRads = FindContentRecursively(curKanjiList + [theKanji], curRadicalSet.union(newRads))
					if newKanji is not -1 and len(newKanji) > minLevelKanji:
						return newKanji, newRads	
				

if len(sys.argv) > 1:
	print("opening " + sys.argv[1])
	inputFile = codecs.open(sys.argv[1], encoding='utf-8')
	#GenFromKanji(inputFile.readline().split()[1])
	startingLiteral = inputFile.readline().split()[1];
	FindContentRecursively([startingLiteral], set(FindKanjiRadicals(startingLiteral))) 
else:
	print("No file given")
	response = raw_input("Generate level from joyo? Y/N")
	if response is "y" or response is "Y" or response is "yes" or response is "Yes":
		inputFile = codecs.open("joyo.txt", encoding='utf-8')
		GenFromFile(inputFile)
	else:
		response = ("Generate from kanji?")
		if response.lower() is "y" or response.lower() is "yes":
			GenFromKanji("本")
		else:
			print("okay, quitting")
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
			
	

    

