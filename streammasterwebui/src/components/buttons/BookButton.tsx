import BaseButton, { type ChildButtonProps } from './BaseButton'

const BookButton: React.FC<ChildButtonProps> = ({
  disabled = false,
  iconFilled = true,
  label,
  onClick,
  tooltip = '',
}) => {
  return (
    <BaseButton
      disabled={disabled}
      icon="pi-book"
      iconFilled={iconFilled}
      label={label}
      onClick={onClick}
      tooltip={tooltip}
    />
  )
}

export default BookButton
