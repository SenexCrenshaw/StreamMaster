import { LinkButton } from '../../buttons/LinkButton';
import getRecordString from '../helpers/getRecordString';

export function epgLinkTemplate(data: object) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'XMLLink')} title="EPG Link" />
      <LinkButton bolt link={getRecordString(data, 'ShortEPGLink')} title="Unsecure EPG Link" />
    </div>
  );
}
