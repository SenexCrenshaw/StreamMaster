import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const XButton: React.FC<ChildButtonProperties> = ({ buttonDisabled, iconFilled, label, onClick, tooltip }) => {
  return (
    <SMButton
      buttonClassName="icon-red"
      buttonDisabled={buttonDisabled}
      icon="pi-times"
      iconFilled={iconFilled}
      label={iconFilled === true ? undefined : label ?? undefined}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default XButton;
