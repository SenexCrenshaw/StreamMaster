import { LinkButton } from '../../buttons/LinkButton';
import getRecordString from '../helpers/getRecordString';

export function urlTemplate(data: object) {
  return (
    <div className="flex justify-content-center align-items-center gap-1">
      <LinkButton filled link={getRecordString(data, 'HDHRLink')} title="HDHR Link" />
    </div>
  );
}
