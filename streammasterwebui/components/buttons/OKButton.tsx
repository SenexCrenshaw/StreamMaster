import { type ChildButtonProps } from './BaseButton';
import BaseButton from './BaseButton';

const OKButton: React.FC<ChildButtonProps> = ({ iconFilled = true, label, onClick, tooltip = '' }) => {
  return (
    <BaseButton
      icon="pi-check"
      iconFilled={iconFilled}
      label={iconFilled !== true ? undefined : label ? label : 'Ok'}
      onClick={onClick}
      rounded={iconFilled}
      severity="success"
      tooltip={tooltip}
    />
  );
};

export default OKButton;
