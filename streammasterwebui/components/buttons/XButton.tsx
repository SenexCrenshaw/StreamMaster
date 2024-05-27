import SMButton from '@components/sm/SMButton';
import { ChildButtonProperties } from './ChildButtonProperties';

const XButton: React.FC<ChildButtonProperties> = ({ disabled, iconFilled, label, onClick, tooltip }) => {
  return (
    <SMButton
      className="icon-red"
      disabled={disabled}
      icon="pi-times"
      iconFilled={iconFilled}
      label={iconFilled === true ? undefined : label ?? undefined}
      onClick={onClick}
      tooltip={tooltip}
    />
  );
};

export default XButton;
