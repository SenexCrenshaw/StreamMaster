import { LinkButton } from '../../buttons/LinkButton';
import getRecordString from '../helpers/getRecordString';

export function epgLinkTemplate(data: object) {
  if (data.hasOwnProperty('XMLLink') === false) {
    return;
  }
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'XMLLink')} title="EPG Link" />
      <LinkButton bolt link={getRecordString(data, 'ShortEPGLink')} title="Short EPG Link" />
    </div>
  );
}
