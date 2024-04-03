import { Column } from 'primereact/column';
import { DataTable } from 'primereact/datatable';

const TestPanel = () => {
  const data = [
    {
      id: '6781',
      code: 'mZ693iN16',
      name: 'House Address',
      description: 'Product Description',
      image: 'strategy.jpg',
      price: 431.95,
      category: 'Housewares',
      quantity: 34,
      inventoryStatus: 'INSTOCK',
      rating: 2
    },
    {
      id: '3937',
      code: 'QX240NF67',
      name: 'Break Even',
      description: 'Product Description',
      image: 'claim.jpg',
      price: 371.46,
      category: 'Clothing',
      quantity: 15,
      inventoryStatus: 'INSTOCK',
      rating: 5
    },
    {
      id: '2068',
      code: 'rW433CF19',
      name: 'Bed Night',
      description: 'Product Description',
      image: 'similar.jpg',
      price: 358.79,
      category: 'Electronics',
      quantity: 55,
      inventoryStatus: 'INSTOCK',
      rating: 2
    },
    {
      id: '4367',
      code: 'mj293Dx85',
      name: 'World Look',
      description: 'Product Description',
      image: 'political.jpg',
      price: 91.15,
      category: 'Accessories',
      quantity: 90,
      inventoryStatus: 'LOWSTOCK',
      rating: 2
    },
    {
      id: '8030',
      code: 'FN322vu49',
      name: 'Dark Thank',
      description: 'Product Description',
      image: 'prevent.jpg',
      price: 270.23,
      category: 'Accessories',
      quantity: 27,
      inventoryStatus: 'LOWSTOCK',
      rating: 3
    },
    {
      id: '1177',
      code: 'wp104Ar86',
      name: 'Note Market',
      description: 'Product Description',
      image: 'writer.jpg',
      price: 188.44,
      category: 'Clothing',
      quantity: 95,
      inventoryStatus: 'INSTOCK',
      rating: 4
    },
    {
      id: '2334',
      code: 'rO076UQ19',
      name: 'Drug Country',
      description: 'Product Description',
      image: 'bank.jpg',
      price: 59.15,
      category: 'Books',
      quantity: 40,
      inventoryStatus: 'LOWSTOCK',
      rating: 4
    },
    {
      id: '4390',
      code: 'ry548Wk48',
      name: 'Read Decade',
      description: 'Product Description',
      image: 'southern.jpg',
      price: 181.71,
      category: 'Housewares',
      quantity: 41,
      inventoryStatus: 'OUTOFSTOCK',
      rating: 2
    },
    {
      id: '5329',
      code: 'hZ165jW53',
      name: 'Difficult Poor',
      description: 'Product Description',
      image: 'rest.jpg',
      price: 48.69,
      category: 'Clothing',
      quantity: 57,
      inventoryStatus: 'OUTOFSTOCK',
      rating: 1
    },
    {
      id: '8960',
      code: 'wj889fZ43',
      name: 'Role Decide',
      description: 'Product Description',
      image: 'office.jpg',
      price: 309.62,
      category: 'Books',
      quantity: 23,
      inventoryStatus: 'OUTOFSTOCK',
      rating: 1
    }
  ];
  return (
    <div className="flex flex-row grid grid-nogutter">
      <DataTable showGridlines value={data} tableStyle={{ minWidth: '50rem' }} scrollable scrollHeight="400px" paginator rows={5}>
        <Column field="code" header="Code" style={{ width: '25%' }}></Column>
        <Column field="name" header="Name" style={{ width: '25%' }}></Column>
        <Column field="category" header="Category" style={{ width: '25%' }}></Column>
        <Column field="quantity" header="Quantity" style={{ width: '25%' }}></Column>
      </DataTable>
    </div>
  );
};
export default TestPanel;
