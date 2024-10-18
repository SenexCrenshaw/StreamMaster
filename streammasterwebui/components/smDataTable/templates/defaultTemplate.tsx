import { camel2title } from '@lib/common/common';
import getRecord from '../helpers/getRecord';

export function defaultTemplate(data: object, fieldName: string, camelize?: boolean, dataKey?: string) {
  const record = getRecord({ data, dataKey, fieldName });
  let recordJson = JSON.stringify(record);
  // Logger.debug('defaultTemplate', {
  //   fieldName: fieldName,
  //   record: record,
  //   recordJson: recordJson,
  //   data: data
  // });
  if (!recordJson) {
    recordJson = '';
  }
  if (recordJson.startsWith('"') && recordJson.endsWith('"')) {
    recordJson = recordJson.substring(1, recordJson.length - 1);
  }

  if (camelize) {
    recordJson = camel2title(recordJson);
  }

  return <div className="text-container px-1">{recordJson}</div>;
}
