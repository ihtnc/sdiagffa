import { describe, expect, it } from 'vitest';
import { render } from '@testing-library/react';
import Header from '.'

describe('Header component', () => {
  it('should match snapshot', () => {
    const component = render(<Header />);
    expect(component).toMatchSnapshot();
  })
});