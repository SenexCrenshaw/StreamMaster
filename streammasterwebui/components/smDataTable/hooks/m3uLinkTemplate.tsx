import { StreamGroupDto } from '@lib/smAPI/smapiTypes';
import { LinkButton } from '../../buttons/LinkButton';
import getRecordString from '../helpers/getRecordString';

export function m3uLinkTemplate(data: StreamGroupDto) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'M3ULink')} title="M3U Link" />
      <LinkButton bolt link={getRecordString(data, 'ShortM3ULink')} title="Short M3U Link" />
    </div>
  );
}
