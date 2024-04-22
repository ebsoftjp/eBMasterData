function doGet(e) {
  const res = getAllData().filter(v => v.Name != "SprData");
  Logger.log(JSON.stringify(res, null, "\t"));

  var output = ContentService.createTextOutput();
  output.setMimeType(ContentService.MimeType.JSON);
  output.setContent(JSON.stringify(res));
  return output;
}

function getAllData() {
  return SpreadsheetApp
    .getActive()
    .getSheets()
    .map(sheet => ({
      Name: sheet.getName(),
      Text: getSheetData(sheet)
        .map(v => v.join(","))
        .join("\r\n"),
    }));
}

function getSheetData(sheet) {
  const rows = [...Array(sheet.getLastRow())].map((_, v) => v + 1);
  const cols = [...Array(sheet.getLastColumn())].map((_, v) => v + 1);
  return rows.map(y => cols.map(x => {
    let v = sheet.getRange(y, x).getValue().toString();
    if (v.includes("\"") || v.includes(",") || v.includes("\n")) {
      v = `"${v.replaceAll("\"", "\"\"")}"`;
    }
    return v;
  }));
}
