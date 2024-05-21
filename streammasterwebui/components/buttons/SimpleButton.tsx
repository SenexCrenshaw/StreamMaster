import SMButton from '@components/sm/SMButton';
import { useIsTrue } from '@lib/redux/hooks/isTrue';

interface SMSimpleButtonProperties {
  dataKey: string;
}

const SimpleButton = ({ dataKey }: SMSimpleButtonProperties) => {
  const { isTrue, setIsTrue } = useIsTrue(dataKey);

  return (
    <SMButton
      icon="pi-window-maximize"
      iconFilled
      className="sm-simple-button icon-blue"
      onClick={() => {
        console.log('isSimple', !isTrue);
        setIsTrue(!isTrue);
      }}
    />
  );
};

export default SimpleButton;
