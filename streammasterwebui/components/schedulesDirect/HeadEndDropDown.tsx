import SMPopUp from '@components/sm/SMPopUp';
import SchedulesDirectHeadendDataSelector from './SchedulesDirectHeadendDataSelector';

interface HeadEndDropDownProps {}

const HeadEndDropDown = (props: HeadEndDropDownProps) => {
  return (
    <SMPopUp contentWidthSize="6" icon="pi-plus" iconFilled label="Lineups">
      <SchedulesDirectHeadendDataSelector />
    </SMPopUp>
  );
};

export default HeadEndDropDown;
