export default {
  "*.java": (filenames) =>
    filenames.map(
      (filename) =>
        `java -jar ../checkstyle-10.15.0-all.jar -c=config/checkstyle/checkstyle.xml '${filename}'`,
    ),
};
