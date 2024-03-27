import { StreamGroupDto } from '@lib/iptvApi';
import { LinkButton } from '../buttons/LinkButton';
import getRecordString from './getRecordString';

export function m3uLinkTemplate(data: StreamGroupDto) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'm3ULink')} title="M3U Link" />
      <LinkButton bolt link={getRecordString(data, 'shortM3ULink')} title="Unsecure M3U Link" />
    </div>
  );
}
