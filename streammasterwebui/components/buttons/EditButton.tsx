import { type ChildButtonProps } from './BaseButton';
import BaseButton from './BaseButton';

const EditButton: React.FC<ChildButtonProps> = ({ disabled = false, iconFilled, label, onClick, tooltip = 'Edit' }) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-pencil"
      iconFilled={iconFilled}
      label={iconFilled === true ? undefined : label}
      onClick={onClick}
      severity="warning"
      tooltip={tooltip}
    />
  );
};

export default EditButton;
