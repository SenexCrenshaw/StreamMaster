import { GetEPGNextEPGNumber } from '@lib/smAPI/EPGFiles/EPGFilesCommands';
import { createAsyncThunk } from '@reduxjs/toolkit';


export const fetchGetEPGNextEPGNumber = createAsyncThunk('cache/getGetEPGNextEPGNumber', async (_: void, thunkAPI) => {
  try {
    console.log('Fetching GetEPGNextEPGNumber');
    const response = await GetEPGNextEPGNumber();
    console.log('Fetched GetEPGNextEPGNumber',response);
    return {param: _, value: response };
  } catch (error) {
    console.error('Failed to fetch', error);
    return thunkAPI.rejectWithValue({ error: error || 'Unknown error', value: undefined });
  }
});


