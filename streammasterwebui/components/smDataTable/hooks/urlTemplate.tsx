import { LinkButton } from '../../buttons/LinkButton';
import getRecordString from '../helpers/getRecordString';

export function urlTemplate(data: object) {
  return <LinkButton filled link={getRecordString(data, 'HDHRLink')} title="HDHR Link" />;
}
