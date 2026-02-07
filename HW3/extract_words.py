## Group 19 Python Word Extractor
import re
from collections import defaultdict

def extractText(path):
    all_words = []
    frequencies = defaultdict(lambda: 0)

    with open(path + "novel.txt", encoding='utf-8') as file:
        lines = file.readlines()
        for line in lines:
            line = line.lower()
            all_words.extend(re.findall(r'[a-z]+', line))

    with open(path + "allwords.txt", 'w', encoding='utf-8') as file:
        for word in all_words:
            file.write(word + '\n')
            frequencies[word] += 1

    with open(path + "uniquewords.txt", 'w', encoding='utf-8') as file:
        for word in frequencies.keys():
            if frequencies[word] == 1:
                file.write(word + '\n')

    with open(path + "wordfrequency.txt", 'w', encoding='utf-8') as file:
        freq_numbers = defaultdict(lambda: 0)
        for key, value in frequencies.items():
            freq_numbers[value] += 1

        freq_numbers = dict(sorted(freq_numbers.items()))

        for key, value in freq_numbers.items():
            file.write(f'{key}: {value}\n')

def main():
    filepath = "./text/"
    extractText(filepath)

if __name__ == "__main__":
    main()