import { LinkButton } from '../buttons/LinkButton';
import getRecordString from './getRecordString';

export function urlTemplate(data: object) {
  return <LinkButton filled link={getRecordString(data, 'hdhrLink')} title="HDHR Link" />;
}
