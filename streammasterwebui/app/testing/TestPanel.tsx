import IconGrid from '@components/icons/IconGrid';

const TestPanel = () => {
  const onClick = (source: string) => {
    console.log(source);
  };

  return (
    <div className="flex-row grid grid-nogutter">
      <IconGrid onClick={onClick} iconSource="/countries%5Cunited-states%5Cus-local%5Cabc-3-katc-horizontal-us.png" />
    </div>
  );
};
export default TestPanel;
