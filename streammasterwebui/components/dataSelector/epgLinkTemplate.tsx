import { LinkButton } from '../buttons/LinkButton';
import getRecordString from './getRecordString';

export function epgLinkTemplate(data: object) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'xmlLink')} title="EPG Link" />
      <LinkButton bolt link={getRecordString(data, 'shortEPGLink')} title="Unsecure EPG Link" />
    </div>
  );
}
