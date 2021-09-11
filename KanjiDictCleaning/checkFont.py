# coding: utf-8
import xml.etree.ElementTree as ET
import codecs
import json
import sys

frequentKanjis = []
fontKanji = []

print("loading xml")
root = ET.parse('kanjidic2.xml').getroot()
print("xml loaded")

print("opening font kanji")
fontFile = codecs.open("fontKanji.txt", encoding='utf-8')


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

def FindKanjiReadings(kanji):
	onyomi, kunyomi = " ", " "
	for reading in kanji.find('reading_meaning').find('rmgroup').findall('reading'):
		if reading.attrib.get("r_type") == "ja_on":
			onyomi += reading.text + "/"
		elif reading.attrib.get("r_type") == "ja_kun":
			kunyomi += reading.text + "/"

	return onyomi.split("/")[:-1], kunyomi.split("/")[:-1]

def FindFrequentKanji():
	for kanji in root.findall('character'):
		freq = kanji.find("misc").find("freq")
		if freq is not None:
			frequentKanjis.append(kanji.find("literal").text)
		
def CheckFontKanji():
	fontKanjiLines = fontFile.readlines()
	missingKanji = []
	numMissing = 0
	for line in fontKanjiLines:
		for chara in line:
			if chara == u"、":
				continue
			else:
				fontKanji.append(chara)
			#if chara not in frequentKanjis:
			#	numMissing += 1
			#else:
			#	print(chara + " found!")
	for kanji in frequentKanjis:
		if kanji not in fontKanji:
			print(kanji)
			kanjiEntry = GetKanjiEntry(kanji)
			if kanjiEntry.find("misc").find("jlpt") is not None:
				print("jlpt level: " + str(kanjiEntry.find("misc").find("jlpt").text))
			missingKanji.append(kanji)
			numMissing += 1
	print(missingKanji)
	print(numMissing)
			
FindFrequentKanji()
CheckFontKanji()
