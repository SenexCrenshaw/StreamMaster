import AddButton from '@components/buttons/AddButton';
import { Card } from 'primereact/card';
import { useRef, useState } from 'react';

type TemplateProps<T> = {
  data: T;
};

function ExpandTemplate<T>({ data }: TemplateProps<T>) {
  // State to control the visibility of the Card
  const [isCardVisible, setCardVisible] = useState(false);
  const op = useRef(null); // Assuming you have a specific use for this ref

  // Function to toggle the visibility of the Card
  const toggleCardVisibility = () => {
    setCardVisible((prevVisible) => !prevVisible);
  };

  return (
    <div className="flex justify-content-between align-items-center p-0 m-0 pl-1">
      <AddButton
        iconFilled={false}
        onClick={(e) => {
          console.log('click');
          toggleCardVisibility(); // Toggle visibility on click
        }}
        ref={op}
        tooltip="Streams"
      />
      {isCardVisible && ( // Render Card only if isCardVisible is true
        <Card title="Simple Card" className="block">
          {' '}
          {/* Changed className to "block" or another class that makes it visible */}
          <p className="m-0">
            Lorem ipsum dolor sit amet, consectetur adipisicing elit. Inventore sed consequuntur error repudiandae numquam deserunt quisquam repellat libero
            asperiores earum nam nobis, culpa ratione quam perferendis esse, cupiditate neque quas!
          </p>
        </Card>
      )}
    </div>
  );
}

export default ExpandTemplate;
