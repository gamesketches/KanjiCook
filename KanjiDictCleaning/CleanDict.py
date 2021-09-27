# coding: utf-8
import xml.etree.ElementTree as ET
import codecs
import json
import sys

maxLevelRadicals = 7
maxLevelKanji = 5
minLevelKanji = 4
targetJLPT = 4

levelCounter = 1
unusableKanji = [];

print("loading xml")
root = ET.parse('kanjidic2.xml').getroot()
print("xml loaded")

print("opening krad")
krad = codecs.open("kradfile-u.txt", encoding='utf-8')
print("krad opened")
recipes = codecs.open("jlpt5krad.txt", encoding='utf-8')

bannedKanji = []

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
	for radEntry in recipes:
		if radEntry[0] == kanji:
			returnVal = radEntry[4:].rstrip().split(" ")
			return returnVal
	
	for radEntry in krad:
		if radEntry[0] == kanji:
			krad.seek(0)
			returnVal = radEntry[4:].rstrip().split(" ")
			if kanji in returnVal:
				returnVal.remove(kanji)
			return returnVal
	
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
	radicalString = ""
	radicalContents = []
	for kanji in cleanedEntries:
		for radical in kanji["radicals"]:
			if radical not in radicalContents:
				radicalString += " " + radical
				#print(radical)
				radicalContents.append(radical)
	
	print(radicalString)
	if len(radicalContents) > maxLevelRadicals:
		print("WARNING: TOO MANY RADS!")
	return radicalString

def GenFromFile(inputFile):
	splitInputFile = inputFile.readline().split()
	theKanji = []
	for i in splitInputFile:
		kanjiEntry = GetKanjiEntry(i)
		foundMeanings = GenKanjiInfoString(kanjiEntry)
		foundMeanings = [var.lstrip() for var in foundMeanings if var]
		foundRadicals = FindKanjiRadicals(i) 
		onyomi, kunyomi = FindKanjiReadings(kanjiEntry)
		theKanji.append( {'kanji' : i, "meanings" : foundMeanings, "radicals" : foundRadicals, "onyomi" : onyomi, "kunyomi" : kunyomi})

	outputFile = open("./LevelOutput/output.json", "w")
	outputFile.writelines(json.dumps({"kanjiInfos" : theKanji}))
	outputFile.close()
	print(json.dumps({"kanjiInfos" : theKanji}))
	CheckRadicalCount(theKanji)

def GenFromList(inputList):
	global levelCounter
	theKanji = []
	kanjiString = "" 
	for i in inputList:
		kanjiEntry = GetKanjiEntry(i)
		kanjiString += " " + i
		foundMeanings = GenKanjiInfoString(kanjiEntry)
		foundMeanings = [var.lstrip() for var in foundMeanings if var]
		foundRadicals = FindKanjiRadicals(i) 
		onyomi, kunyomi = FindKanjiReadings(kanjiEntry)
		theKanji.append( {'kanji' : i, "meanings" : foundMeanings, "radicals" : foundRadicals, "onyomi" : onyomi, "kunyomi" : kunyomi})

	outputFile = open("./LevelOutput/level" + str(levelCounter) + ".json", "w")
	outputFile.writelines(json.dumps({"kanjiInfos" : theKanji}))
	outputFile.close()
	print(json.dumps({"kanjiInfos" : theKanji}))
	print(kanjiString)
	levelListFile = codecs.open("./LevelList.csv", mode="w", encoding='utf-8')
	levelListFile.write("level" + str(levelCounter) + "," + kanjiString + "," + CheckRadicalCount(theKanji) + "\n")
	levelListFile.close()
	#CheckRadicalCount(theKanji)
	levelCounter += 1

def ProcessKanjiForRecursion(newKanji, curKanjiList, curRadicalSet):
	newRads = FindKanjiRadicals(newKanji)
	numIntersection = len(curRadicalSet.intersection(newRads))
	if numIntersection > 0:
		radsAdded = len(newRads) - numIntersection
		if len(curRadicalSet) + radsAdded <= maxLevelRadicals:
			if len(curKanjiList) + 1 == maxLevelKanji:
				print(len(curRadicalSet) + radsAdded)
				GenFromList(curKanjiList + [newKanji])
				return curKanjiList + [newKanji], curRadicalSet.union(newRads)
			else:
				print("going a level deeper on " + newKanji)
				newKanji, newRads = FindContentRecursively(curKanjiList + [newKanji], curRadicalSet.union(newRads))
				if newKanji[0] is not -1 and len(newKanji) > minLevelKanji:
					return newKanji, newRads
	return [-1, -1], set([])

def FindContentRecursively(curKanjiList, curRadicalSet):
	global maxLevelKanji, maxLevelRadicals, minLevelKanji
	checkAfterIteration = []
	for kanji in root.findall('character'):
		freq = kanji.find("misc").find("freq")
		if freq is None:
			root.remove(kanji)
			continue
		jlptLevel = kanji.find("misc").find("jlpt")
		acceptableJLPT = jlptLevel is not None and jlptLevel >= targetJLPT
		theKanji = kanji.find("literal").text
		if theKanji in curKanjiList or theKanji in unusableKanji or acceptableJLPT:
			continue
		if jlptLevel > targetJLPT:
			checkAfterIteration.append(theKanji)
			continue
		newKanji, newRads = ProcessKanjiForRecursion(theKanji, curKanjiList, curRadicalSet)
		if newKanji[0] is not -1 and len(newKanji) > minLevelKanji:
			return newKanji, newRads
		#newRads = FindKanjiRadicals(theKanji)
		#numIntersection = len(curRadicalSet.intersection(newRads))
		#if numIntersection > 0:
		#	radsAdded = len(newRads) - numIntersection
		#	if len(curRadicalSet) + radsAdded <= maxLevelRadicals:
		#		if len(curKanjiList) + 1 == maxLevelKanji:
		#			print(len(curRadicalSet) + radsAdded)
		#			GenFromList(curKanjiList + [theKanji])
		#			return curKanjiList + [theKanji], curRadicalSet.union(newRads)
		#		else:
		#			print(".")
		#			newKanji, newRads = FindContentRecursively(curKanjiList + [theKanji], curRadicalSet.union(newRads))
		#			if newKanji[0] is not -1 and len(newKanji) > minLevelKanji:
		#				return newKanji, newRads	
	print("Looking through leftovers: " + str(len(checkAfterIteration)))
	for leftOver in checkAfterIteration:
		newKanji, newRads = ProcessKanjiForRecursion(leftOver, curKanjiList, curRadicalSet)
		if newKanji[0] is not -1 and len(newKanji) > minLevelKanji:
			return newKanji, newRads
	return [-1, -1], set([])
				

bannedKanjiFile = codecs.open("bannedKanji.txt", encoding='utf-8')
bannedKanjiLine = bannedKanjiFile.readline()
for bannedKanji in bannedKanjiLine.split():
	unusableKanji.append(bannedKanji)
	
print("Banned Kanji List: ")
print(bannedKanji)

if len(sys.argv) > 1:
	print("opening " + sys.argv[1])
	inputFile = codecs.open(sys.argv[1], encoding='utf-8')
	kanjiLines = inputFile.readlines()
	for inputKanjis in kanjiLines:
		for literal in inputKanjis.split():
			print("building for " + literal)
			FindContentRecursively([literal], set(FindKanjiRadicals(literal)))
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
			
	

    

