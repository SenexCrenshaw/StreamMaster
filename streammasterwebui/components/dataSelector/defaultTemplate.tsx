import { camel2title } from '@lib/common/common';
import getRecord from '../smDataTable/helpers/getRecord';

export function defaultTemplate(data: object, fieldName: string, camelize?: boolean) {
  let recordJson = JSON.stringify(getRecord(data, fieldName));

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
